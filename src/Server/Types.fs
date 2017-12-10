namespace TimeOff.Server

open Shared.Types

/// Represents the rights available for a request
/// This is the type that gets stored in the JsonWebToken
type UserRights = {
  UserName : string
  Role: UserRole }