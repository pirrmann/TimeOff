module Client.Employees.Rest

open Fable.PowerPack
open Fable.PowerPack.Fetch

open Shared.Types
open Client

let getEmployees token () =
    promise {
        let props = propsOfToken token
        return! fetchAs<Employee list> Shared.ServerUrls.Employees props
    }

let getEmployee token userName =
    promise {
        let props = propsOfToken token
        return! fetchAs<Employee> (Shared.ServerUrls.Employees + userName) props
    }

let createEmployee token (employee: Employee) =
    promise {
        let props = propsOfToken token
        let! response = postRecord (Shared.ServerUrls.Employees + employee.UserName) employee props
        return! parseAs<Employee> response
    }

let updateEmployee token (employee: Employee) =
    promise {
        let props = propsOfToken token
        let! response = putRecord (Shared.ServerUrls.Employees + employee.UserName) employee props
        return! parseAs<Employee> response
    }
