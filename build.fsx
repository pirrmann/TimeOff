// include Fake libs
#r "./packages/build/FAKE/tools/FakeLib.dll"
#r "./packages/build/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"

open System
open System.IO
open Fake
open Fake.YarnHelper

let dotnetcliVersion : string = 
    try
        let content = File.ReadAllText "global.json"
        let json = Newtonsoft.Json.Linq.JObject.Parse content
        let sdk = json.Item("sdk") :?> Newtonsoft.Json.Linq.JObject
        let version = sdk.Property("version").Value.ToString()
        version
    with
    | exn -> failwithf "Could not parse global.json: %s" exn.Message

let mutable dotnetExePath = "dotnet"

let runDotnet workingDir args =
    let result =
        ExecProcess (fun info ->
            info.FileName <- dotnetExePath
            info.WorkingDirectory <- workingDir
            info.Arguments <- args) TimeSpan.MaxValue
    if result <> 0 then failwithf "dotnet %s failed" args

Target "InstallDotNetCore" (fun _ ->
    dotnetExePath <- DotNetCli.InstallDotNetSDK dotnetcliVersion
)

// --------------------------------------------------------------------------------------
// Targets
// --------------------------------------------------------------------------------------

Target "Clean" (fun _ ->
    !! "/src/**/bin"
    ++ "/src/**/obj"
    ++ "/tests/**/obj"
    ++ "/tests/**/obj"
  |> CleanDirs
)

Target "Restore" (fun _ ->
    runDotnet __SOURCE_DIRECTORY__ "restore TimeOff.sln"
)

Target "YarnInstall" (fun _ ->
    Yarn (fun p ->
            { p with
                Command = Install Standard
            })
)

Target "BuildClient" (fun _ ->
    runDotnet "src/Client" """fable yarn-build"""
)

Target "BuildServer" (fun _ ->
    runDotnet "src/Server" """build"""
)

let watchClient = async {
    runDotnet "src/Client" """fable yarn-start"""
}

let watchServer = async {
    runDotnet "src/Server" """watch run"""
}

let openBrowser = async {
    do! Async.Sleep 5000
    Diagnostics.Process.Start("http://localhost:8080/") |> ignore
}

Target "WatchClient" (fun _ ->
    watchClient |> Async.RunSynchronously
)

Target "WatchServer" (fun _ ->
    watchServer |> Async.RunSynchronously    
)

Target "Run" (fun _ ->
    Async.Parallel [| watchClient; watchServer; openBrowser |]
    |> Async.RunSynchronously
    |> ignore
)

Target "BuildServerTests" (fun _ ->
    runDotnet "tests/ServerTests" """build"""
)

Target "RunServerTests" (fun _ ->
    runDotnet "tests/ServerTests" "run"
)

// --------------------------------------------------------------------------------------
// Build order
// --------------------------------------------------------------------------------------

Target "All" DoNothing

Target "RunAllTests" DoNothing

Target "Build" DoNothing

"Clean"
  ==> "InstallDotNetCore"
  ==> "Restore"

"Restore"
  ==> "YarnInstall"
  ==> "BuildClient"

"YarnInstall"
  ==> "WatchClient"

"Restore" ==> "BuildServer"
"Restore" ==> "WatchServer"

"BuildClient" ==> "Build"
"BuildServer" ==> "Build"

"Build" ==> "Run"

"Build"
  ==> "BuildServerTests"
  ==> "RunServerTests"
  ==> "RunAllTests"
  ==> "All"

RunTargetOrDefault "All"
