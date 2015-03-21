﻿open System

open Orleankka
open Orleankka.FSharp
open Orleankka.FSharp.FuncActor

open Shop
open Account

[<EntryPoint>]
let main argv = 

   printfn "Running demo. Booting cluster might take some time ...\n"
   
   use system = playgroundActorSystem()
                |> registerFuncActors <@[ShopActor; AccountActor]@>
                |> start

   let shop = spawn system ShopActor "Amazon"
   let account = spawn system AccountActor "Antya"

   task {

      let! stock = shop <? Stock
      printfn "Shop has %i items in stock \n" stock

      let! balance = account <? Balance
      printfn "Account balance is %i \n" balance

      printfn "Let's put 100$ on the account \n"
      do! account <? Deposit(100)      

      printfn "Let's put 5 items in stock \n"
      do! shop <? CheckIn(5)

      let! stock = shop <? Stock
      printfn "Now shop has %i items in stock \n" stock

      printfn "Let's sell 2 items to user \n"
      do! shop <? Sell(account, 2)      

      let! stock = shop <? Stock
      printfn "Now shop has %i items in stock \n" stock

      let! balance = account <? Balance
      printfn "And account balance is %i \n" balance
   }
   |> Task.wait

   Console.ReadLine() |> ignore

   printfn "%A" argv
   0 // return an integer exit code
