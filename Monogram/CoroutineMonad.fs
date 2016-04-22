module CoroutineMonad

type Coroutine<'a,'s> = 's -> CoroutineStep<'a,'s>
and CoroutineStep<'a,'s> =
  | Done of 'a*'s
  | Yield of Coroutine<'a,'s>*'s

let ret x = fun s -> Done(x,s)

let rec bind p k =
  fun s ->
    match p s with
    | Done(a, s') -> k a s'
    | Yield(p', s') -> Yield((bind p' k), s')

let rec Run c s =
  match c s with
  | Done(result, s') -> result
  | Yield(c', s') -> Run c' s'

let rec RunAll c s =
  match c s with
  | Done(result, s') -> result, s'
  | Yield(c', s') -> RunAll c s

type CoroutineBuilder() =
  member this.Return(x) = ret x
  member this.ReturnFrom(c) = c
  member this.Bind(p,k) = bind p k
  member this.Zero() = ret ()
  member this.For(items:seq<'a>, body:'a -> Coroutine<Unit,'s>) : Coroutine<Unit,'s> =
    this.ForList(items |> Seq.toList, body)
  member this.ForList(items:list<'a>, body:'a -> Coroutine<Unit,'s>) : Coroutine<Unit,'s> =
    fun s ->
      match items with
      | [] -> Done((),s)
      | hd :: tl -> 
        this.Combine(body hd, this.ForList(tl, body)) s
  member this.Combine(p,k) =
    fun (s:'s) ->
      match p s with
      | Done(_,s') -> k s'
      | Yield(p', s') -> Yield(this.Combine(p', k), s')
  member this.Delay s = this.Bind(this.Return(), s)
  
let cor = CoroutineBuilder()

let yield_ = fun s -> Yield((fun s -> Done((), s)),s)

let print_ x = fun s -> printfn "%A" x
                        Done((), s)

let rec repeat_ x =
  cor{
    do! x
    return! repeat_ x
  }

let wait_ interval =
  let getTime = fun s -> Done(System.DateTime.Now, s)
  cor{
    let! t0 = getTime
    let rec wait_() =
      cor{
        let! tc = getTime
        let dt = (tc-t0).TotalSeconds
        if dt > interval then
          return ()
        else
          do! yield_
          return! wait_()
      }
    do! wait_()
  }

let costep c =
  match c with
  | Done(a, s')   -> (ret a), s'
  | Yield(c', s') -> c', s'