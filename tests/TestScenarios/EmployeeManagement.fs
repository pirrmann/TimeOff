module TestsScenarios.EmployeeManagement

  open Shared.Types

  type Given =
    | ``An employee already exists`` of Employee

  type When =
    | ``Create an employee`` of Employee

  type Then =
    | ``The employee exists`` of Employee
    | ``There was an error``

  let scenarios = [
    {
      Name = "Create first employee"
      Given = []
      When = ``Create an employee`` { UserName = "jdoe"; FirstName = "John"; LastName = "Doe" }
      Then = [``The employee exists`` { UserName = "jdoe"; FirstName = "John"; LastName = "Doe" }]
    }

    {
      Name = "Create second employee"
      Given = [``An employee already exists`` { UserName = "jdoe"; FirstName = "John"; LastName = "Doe" }]
      When = ``Create an employee`` { UserName = "barctor"; FirstName = "Bob"; LastName = "Arctor" }
      Then = [``The employee exists`` { UserName = "barctor"; FirstName = "Bob"; LastName = "Arctor" }]
    }

    {
      Name = "Create employee with already used UserName"
      Given = [``An employee already exists`` { UserName = "jdoe"; FirstName = "John"; LastName = "Doe" }]
      When = ``Create an employee`` { UserName = "jdoe"; FirstName = "Jane"; LastName = "Doe" }
      Then =
        [
          ``There was an error``
          ``The employee exists`` { UserName = "jdoe"; FirstName = "John"; LastName = "Doe" }
        ]
    }
  ]
