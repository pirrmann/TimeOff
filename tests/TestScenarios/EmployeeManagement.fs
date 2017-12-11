module TestsScenarios.EmployeeManagement

open System
open Shared.Types
open Suave.Logging

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
    When = ``Create an employee`` { UserName = "jdoe"; FirstName = "John"; LastName = "Doe"; StartDate = DateTime(2017, 12, 1); MonthlyVacationRate = 2. }
    Then = [``The employee exists`` { UserName = "jdoe"; FirstName = "John"; LastName = "Doe"; StartDate = DateTime(2017, 12, 1); MonthlyVacationRate = 2. }]
  }

  {
    Name = "Create second employee"
    Given = [``An employee already exists`` { UserName = "jdoe"; FirstName = "John"; LastName = "Doe"; StartDate = DateTime(2017, 12, 1); MonthlyVacationRate = 2. }]
    When = ``Create an employee`` { UserName = "barctor"; FirstName = "Bob"; LastName = "Arctor"; StartDate = DateTime(2017, 12, 1); MonthlyVacationRate = 2. }
    Then = [``The employee exists`` { UserName = "barctor"; FirstName = "Bob"; LastName = "Arctor"; StartDate = DateTime(2017, 12, 1); MonthlyVacationRate = 2. }]
  }

  {
    Name = "Create employee with already used UserName"
    Given = [``An employee already exists`` { UserName = "jdoe"; FirstName = "John"; LastName = "Doe"; StartDate = DateTime(2017, 12, 1); MonthlyVacationRate = 2. }]
    When = ``Create an employee`` { UserName = "jdoe"; FirstName = "Jane"; LastName = "Doe"; StartDate = DateTime(2017, 12, 1); MonthlyVacationRate = 2. }
    Then =
      [
        ``There was an error``
        ``The employee exists`` { UserName = "jdoe"; FirstName = "John"; LastName = "Doe"; StartDate = DateTime(2017, 12, 1); MonthlyVacationRate = 2. }
      ]
  }
]
