namespace TestsScenarios

open Expecto

type Scenario<'Given, 'When, 'Then> = {
  Name: string
  Given: 'Given list
  When: 'When
  Then: 'Then list
}

type IScenarioRunner<'Given, 'When, 'Then> =
  abstract member Run: scenario: Scenario<'Given, 'When, 'Then> -> Test

[<AutoOpen>]
module BuildTests =
  let buildTestsFromScenarios groupName (runner: IScenarioRunner<_, _, _>) scenarios =
    scenarios
    |> List.map runner.Run
    |> testList groupName

