module ServerTests.AllTests

open Expecto
open TestsScenarios

open Shared.Types

module EmployeeManagementOnRepository =
  open EmployeeManagement

  let runner =
    let run scenario = testCaseAsync scenario.Name <| async {
      let employeeRepository = ProtoPersist.InMemory.InMemoryRepository.Create<string, Employee>()
      
      for given in scenario.Given do
        match given with
        | ``An employee already exists`` employee ->
          do! employeeRepository.Create employee.UserName employee |> Async.Ignore

      let mutable error = false
      try
        match scenario.When with
        | ``Create an employee`` employee ->
          do! employeeRepository.Create employee.UserName employee |> Async.Ignore
       with _ ->
         error <- true

      for ``then`` in scenario.Then do
        match ``then`` with
        | ``The employee exists`` employee ->
          let! employee' = employeeRepository.Read employee.UserName
          Expect.equal employee' (Some employee) "The employee exists"
        | ``There was an error`` ->
          Expect.isTrue error "There was an error"
    }

    { new IScenarioRunner<Given, When, Then> with
      member __.Run(scenario) = run scenario }

module EmployeeManagementOnApi =
  open Suave
  open Suave.Testing
  open TimeOff.Server
  open EmployeeManagement

  let runner =
    let run scenario = testCaseAsync scenario.Name <| async {

      let jwtEncoder = JsonWebTokenEncoder (PassPhrase.CreateNewPassPhrase())
      let userRights = { UserName = "hr"; Role = HumanResources }
      let token = sprintf "Bearer %s" (jwtEncoder.CreateToken userRights)
      let tokenExtractor =
        { new ITokenExtractor with member __.ExtractToken _ = Some token }

      let authentifier = Authentifier(tokenExtractor, jwtEncoder)

      let employeeRepository = ProtoPersist.InMemory.InMemoryRepository.Create<string, Employee>()

      let nonLogger =
        { new Logging.Logger with
            member __.name = [| "NonLogger" |]
            member __.log _ _ = async { return () }
            member __.logWithAck _ _ = async { return () } }

      let suaveTestContext = 
        Server.mainWebPart authentifier employeeRepository
        |> runWith { defaultConfig with logger = nonLogger }

      for given in scenario.Given do
        match given with
        | ``An employee already exists`` employee ->
          do! employeeRepository.Create employee.UserName employee |> Async.Ignore

      let mutable error = false
      try
        match scenario.When with
        | ``Create an employee`` employee ->
          let employeeJson = new System.Net.Http.StringContent (toJson employee)
          let statusCode = reqStatusCode POST (Shared.ServerUrls.Employees + employee.UserName) (Some employeeJson) suaveTestContext
          if statusCode <> System.Net.HttpStatusCode.OK then
            error <- true
       with _ ->
         error <- true

      for ``then`` in scenario.Then do
        match ``then`` with
        | ``The employee exists`` employee ->
          let! employee' = employeeRepository.Read employee.UserName
          Expect.equal employee' (Some employee) "The employee exists"
        | ``There was an error`` ->
          Expect.isTrue error "There was an error"
    }

    { new IScenarioRunner<Given, When, Then> with
      member __.Run(scenario) = run scenario }

let userTests =
  testList "Employee management tests" [
    EmployeeManagement.scenarios |> buildTestsFromScenarios "at repository level" EmployeeManagementOnRepository.runner
    EmployeeManagement.scenarios |> buildTestsFromScenarios "at API level" EmployeeManagementOnApi.runner
  ]

let tests =
  testList "All server tests" [
    userTests
  ]