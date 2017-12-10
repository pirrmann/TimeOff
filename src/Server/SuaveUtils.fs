[<AutoOpen>]
module SuaveUtils

let scanSingleStringFormat pattern = new PrintfFormat<(string -> string),unit,string,string,string>(pattern)
