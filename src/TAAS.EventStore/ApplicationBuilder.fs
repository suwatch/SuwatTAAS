module TAAS.Infrastructure.ApplicationBuilder
open Railroad

let buildApplication save handler c = 
    (handler c) >>= save

