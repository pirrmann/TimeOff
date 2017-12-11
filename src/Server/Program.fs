module TimeOff.Server.Program

open Shared.Types
open ProtoPersist

open Suave

[<EntryPoint>]
let main _ =

  let jwtEncoder = JsonWebTokenEncoder PassPhrase.AutoGenerated 
  let authentifier = Authentifier(TokenExtractor(), jwtEncoder)

  let userRepository =
    FileSystem.DirectoryRepository.Create<string, User>(@"..\..\DB\users", id)
    |> Agent.AgentRepository.Wrap

  let config =
    { defaultConfig with
       bindings = [ HttpBinding.create HTTP System.Net.IPAddress.Loopback 8081us ] }

  Server.mainWebPart authentifier userRepository
  |> startWebServer config

  0