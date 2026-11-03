IT Asset Checkout is a custom equipment-loan scenario, not a course sample.

# Unit tests

The ids below are the same ids as the xUnit methods in `tests/ItAssetCheckout.Tests` and the rows on the in-app Test page. Nothing else is numbered UT-*.

## 目的

Show that the date rule, the one-open-checkout rule, the staff email rule, and the purchase-date rule accept the allowed values and reject the boundary values without starting the web host.

## 方法

From the repository root, run `dotnet test tests/ItAssetCheckout.Tests`. Each method calls either `Validator.TryValidateObject` or `CheckoutRules.IsSecondOpenCheckout`. A case passes only when every assertion in that method holds. `dotnet test` prints the method name, which starts with the case id.

## 用例表

| ID | xUnit method | Input | Assertion text |
|---|---|---|---|
| UT-01 | `CheckoutValidationTests.UT01_DueBeforeCheckout_IsRejected` | Checkout 2024-06-10, due 2024-06-09, no return date | `Assert.Contains` a result whose member is `DueAt` and whose message is `Due date cannot be before the checkout date.` |
| UT-02 | `CheckoutValidationTests.UT02_DueOnCheckoutDate_IsAccepted` | Checkout 2024-06-10, due 2024-06-10, no return date | `Assert.Empty` on the validation results |
| UT-03 | `CheckoutRulesTests.UT03_SecondOpenCheckout_IsRejected` | Open row while another open checkout exists; a returned row while another is open; an open row when none exists | `Assert.True` only for the first call. `Assert.False` for the returned row and for the first open row |
| UT-04 | `StaffValidationTests.UT04_StaffEmail_MustBeValid` | Email `not-an-email`, then `ada.okonkwo@example.com` | `Assert.Contains` the message `Enter a valid email address.` The valid address has `Assert.Empty` |
| UT-05 | `AssetValidationTests.UT05_FuturePurchaseDate_IsRejected` | Purchase date tomorrow, then today | Tomorrow `Assert.Contains` `Purchase date cannot be in the future.` Today `Assert.DoesNotContain` that message |

## 判断

Pass: `dotnet test tests/ItAssetCheckout.Tests` reports 5 passed and 0 failed, and the five method names above are the only UT cases. Fail: any of those methods is reported failed, or a new UT id appears in code without a row in this table.
