module ServerTests.AllTests

open Expecto
open Scenarios

open Shared.Types

module UserManagementOnRepository =
  open UserManagement

  let runner =
    let run scenario = testCaseAsync scenario.Name <| async {
      let userRepository = ProtoPersist.InMemory.InMemoryRepository.Create<string, User>()
      
      for given in scenario.Given do
        match given with
        | ``A user already exists`` user ->
          do! userRepository.Create user.UserName user |> Async.Ignore

      let mutable error = false
      try
        match scenario.When with
        | ``Create a user`` user ->
          do! userRepository.Create user.UserName user |> Async.Ignore
       with _ ->
         error <- true

      for ``then`` in scenario.Then do
        match ``then`` with
        | ``The user exists`` user ->
          let! user' = userRepository.Read user.UserName
          Expect.equal user' (Some user) "The user exists"
        | ``There was an error`` ->
          Expect.isTrue error "There was an error"
    }

    { new IScenarioRunner<Given, When, Then> with
      member __.Run(scenario) = run scenario }

module UserManagementOnApi =
  open Suave
  open Suave.Testing
  open TimeOff.Server
  open UserManagement

  let runner =
    let run scenario = testCaseAsync scenario.Name <| async {

      let jwtEncoder = JsonWebTokenEncoder (PassPhrase.CreateNewPassPhrase())
      let userRights = { UserName = "hr"; Role = HumanResources }
      let token = sprintf "Bearer %s" (jwtEncoder.CreateToken userRights)
      let tokenExtractor =
        { new ITokenExtractor with member __.ExtractToken _ = Some token }

      let authentifier = Authentifier(tokenExtractor, jwtEncoder)

      let userRepository = ProtoPersist.InMemory.InMemoryRepository.Create<string, User>()

      let nonLogger =
        { new Logging.Logger with
            member __.name = [| "NonLogger" |]
            member __.log _ _ = async { return () }
            member __.logWithAck _ _ = async { return () } }

      let suaveTestContext = 
        Server.mainWebPart authentifier userRepository
        |> runWith { defaultConfig with logger = nonLogger }

      for given in scenario.Given do
        match given with
        | ``A user already exists`` user ->
          do! userRepository.Create user.UserName user |> Async.Ignore

      let mutable error = false
      try
        match scenario.When with
        | ``Create a user`` user ->
          let userJson = new System.Net.Http.StringContent (toJson user)
          let statusCode = reqStatusCode POST (Shared.ServerUrls.Users + user.UserName) (Some userJson) suaveTestContext
          if statusCode <> System.Net.HttpStatusCode.OK then
            error <- true
       with _ ->
         error <- true

      for ``then`` in scenario.Then do
        match ``then`` with
        | ``The user exists`` user ->
          let! user' = userRepository.Read user.UserName
          Expect.equal user' (Some user) "The user exists"
        | ``There was an error`` ->
          Expect.isTrue error "There was an error"
    }

    { new IScenarioRunner<Given, When, Then> with
      member __.Run(scenario) = run scenario }

let userTests =
  testList "User management tests" [
    UserManagement.scenarios |> buildTestsFromScenarios "at repository level" UserManagementOnRepository.runner
    UserManagement.scenarios |> buildTestsFromScenarios "at API level" UserManagementOnApi.runner
  ]

let tests =
  testList "All server tests" [
    userTests
  ]