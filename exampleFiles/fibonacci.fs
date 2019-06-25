let fibonacci (n:bigInt) =
  Seq.Init(int n) id
  |> Seq.fold(fun(x, y) items -> (x+y, x)) (0L, 1L)
  |> fst
