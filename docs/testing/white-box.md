IT Asset Checkout is a custom equipment-loan scenario, not a course sample.

# White-box tests

## 目的

Exercise the validation branches: empty required fields, over-long text, a value of the wrong kind, and dates on the boundary. These are the checks inside the models and the create actions, not a tour of the whole screen.

## 方法

With the demo user signed in, POST the create actions and read the validation messages. Dates use `yyyy-MM-dd`. The antiforgery token is taken from the matching create form. Messages below are the strings the validators return.

## 用例表

| ID | Input | Branch | Expected message | Result on 2026-11-03 |
|---|---|---|---|---|
| WB-01 | Asset create with every field empty | Required fields | `Tag number is required.` | Pass |
| WB-02 | Tag number of 40 characters | Over-long string | `maximum length of 32` | Pass |
| WB-03 | Tag number `IT 100` | Illegal characters | `letters, numbers, and hyphens only` | Pass |
| WB-04 | Asset value `-5` | Bad number | `between 0 and 9999999.99` | Pass |
| WB-05 | Purchase date `2099-01-01` | Future date | `Purchase date cannot be in the future.` | Pass |
| WB-06 | Purchase date `1989-12-31` | Date before the floor | `Purchase date cannot be before 1990-01-01.` | Pass |
| WB-07 | Staff email `not-an-email` | Bad email | `Enter a valid email address.` | Pass |
| WB-08 | Staff code `S-1001` again | Duplicate code | `A staff member with this code already exists.` | Pass |
| WB-09 | Checkout due date the day before checkout | Critical date | `Due date cannot be before the checkout date.` | Pass |
| WB-10 | Second open checkout for `IT-10490` | Open-checkout branch | `This asset already has an open checkout.` | Pass |

WB-05 and WB-09 match UT-05 and UT-01. The white-box run posts them through the create actions so the controller path is included.

## 判断

Pass: each POST stays on the create form and the HTML contains the expected message. Fail: the app saves the row, or the message text differs from the table.
