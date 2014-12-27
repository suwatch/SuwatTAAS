namespace TAAS.Infrastructure

module Security =
    open System
    open System.Security.Cryptography

    //http://www.obviex.com/samples/hash.aspx
    let generateSalt = 
        let minSaltSize = 4
        let maxSaltSize = 8
        let random = new Random()
        let saltSize = random.Next(minSaltSize, maxSaltSize)
        let saltBytes = Array.init saltSize (fun _ -> byte 0)
        let rng = new RNGCryptoServiceProvider()
        rng.GetNonZeroBytes(saltBytes) |> ignore
        saltBytes

    let hasher s = 
        let getbytes (s:string) = (System.Text.Encoding.UTF8.GetBytes(s), generateSalt)
        let concatBytes (frontBytes, backBytes) = (frontBytes, backBytes, Array.concat [frontBytes;backBytes])
        let computeHash hashMethod (bytes, saltBytes, allBytes) = (bytes, saltBytes, hashMethod allBytes)
        let addSaltToHash (bytes, saltBytes, allBytes) = Array.concat [allBytes;saltBytes]

        use algorithm = new System.Security.Cryptography.SHA512Managed()
        let hasher (b:byte array) = algorithm.ComputeHash(b)
        s |> (getbytes >> concatBytes >> (computeHash hasher) >> addSaltToHash >> System.Convert.ToBase64String)
