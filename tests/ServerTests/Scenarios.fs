module ServerTests.Scenarios

open Expecto

type Scenario<'Given, 'When, 'Then> = {
  Name: string
  Given: 'Given list
  When: 'When
  Then: 'Then list
}

type IScenarioRunner<'Given, 'When, 'Then> =
  abstract member Run: scenario: Scenario<'Given, 'When, 'Then> -> Test

let buildTestsFromScenarios groupName (runner: IScenarioRunner<_, _, _>) scenarios =
  scenarios
  |> List.map runner.Run
  |> testList groupName

module UserManagement =

  open Shared.Types

  type Given =
    | ``A user already exists`` of User

  type When =
    | ``Create a user`` of User

  type Then =
    | ``The user exists`` of User
    | ``There was an error``

  let scenarios = [
    {
      Name = "Create first user"
      Given = []
      When = ``Create a user`` { UserName = "jdoe"; FirstName = "John"; LastName = "Doe" }
      Then = [``The user exists`` { UserName = "jdoe"; FirstName = "John"; LastName = "Doe" }]
    }

    {
      Name = "Create second user"
      Given = [``A user already exists`` { UserName = "jdoe"; FirstName = "John"; LastName = "Doe" }]
      When = ``Create a user`` { UserName = "barctor"; FirstName = "Bob"; LastName = "Arctor" }
      Then = [``The user exists`` { UserName = "barctor"; FirstName = "Bob"; LastName = "Arctor" }]
    }

    {
      Name = "Create user with same UserName"
      Given = [``A user already exists`` { UserName = "jdoe"; FirstName = "John"; LastName = "Doe" }]
      When = ``Create a user`` { UserName = "jdoe"; FirstName = "Jane"; LastName = "Doe" }
      Then =
        [
          ``There was an error``
          ``The user exists`` { UserName = "jdoe"; FirstName = "John"; LastName = "Doe" }
        ]
    }
  ]
