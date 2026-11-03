IT Asset Checkout is a custom equipment-loan scenario, not a course sample.

# Black-box tests

## 目的

Check the three registers from the outside: a signed-in user can see the seeded rows, search them, and see that a checkout row ties an asset tag to a staff member. The open loan is visible as Open.

## 方法

Start the app with `dotnet run --project src/ItAssetCheckout` and sign in with the demo account. 演示账号见 `.env.example`. Demo environment only. Then read the HTML of the list pages. No knowledge of the private helpers is required. The seeded catalogue is six assets, six staff members, and six checkouts, one of which is open.

## 用例表

| ID | Steps | Expected visible text | Result on 2026-11-03 |
|---|---|---|---|
| BB-01 | Open `/Assets` after sign-in | `IT-10482`, `Dell Latitude 5440`, `IT-10490`, `IT-14007` | Pass |
| BB-02 | Open `/Assets?q=laptop` | `Dell Latitude 5440` is present and `LG 27UP850` is absent | Pass |
| BB-03 | Open `/Staff` | `S-1001`, `Ada Okonkwo`, `S-1006`, `Farid Hassan` | Pass |
| BB-04 | Open `/Staff?q=finance` | `Ada Okonkwo` is present and `Ben Cho` is absent | Pass |
| BB-05 | Open `/Checkouts` | `IT-10490`, `Ben Cho`, `Open`, and `S-1001` on the returned laptop loan | Pass |
| BB-06 | Open `/Checkouts?q=open` | `On the IT bench.` is present and `Finance month-end.` is absent | Pass |

BB-05 is the relation check: the open row names both the asset tag `IT-10490` and the staff member `Ben Cho`. The returned laptop row names both `IT-10482` and `Ada Okonkwo`.

## 判断

Pass: every row in the table was observed in the running app on 2026-11-03 with the seeded database. Fail: a seeded tag, name, or the open badge is missing, or a search returns a row it should hide.
