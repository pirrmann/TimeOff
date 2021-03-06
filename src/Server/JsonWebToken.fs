/// JSON Web Token (JWT) functions.
namespace TimeOff.Server

//  Learn about JWT https://jwt.io/introduction/
//  This module uses the JOSE-JWT library https://github.com/dvsekhvalnov/jose-jwt

open System.IO
open Newtonsoft.Json

module PassPhrase =
    let CreateNewPassPhrase() = 
        let crypto = System.Security.Cryptography.RandomNumberGenerator.Create()
        let randomNumber = Array.init 32 byte
        crypto.GetBytes(randomNumber)
        randomNumber

    let AutoGenerated =
        let fi = FileInfo("./temp/token.txt")
        if not fi.Exists then
            let passPhrase = CreateNewPassPhrase()
            if not fi.Directory.Exists then
                fi.Directory.Create()
            File.WriteAllBytes(fi.FullName, passPhrase)
        File.ReadAllBytes(fi.FullName)

type JsonWebTokenEncoder (passPhrase: byte array) =
    let encodeString (payload: string) =
        Jose.JWT.Encode(payload, passPhrase, Jose.JweAlgorithm.A256KW, Jose.JweEncryption.A256CBC_HS512)

    let decodeString (jwt: string) =
        Jose.JWT.Decode(jwt, passPhrase, Jose.JweAlgorithm.A256KW, Jose.JweEncryption.A256CBC_HS512)

    member __.CreateToken (userRights: UserRights) =
        JsonConvert.SerializeObject userRights |> encodeString

    member __.Validate (jwt: string) : UserRights option =
        try
            let userRights = decodeString jwt |> JsonConvert.DeserializeObject<UserRights>
            Some userRights
        with
        | _ -> None