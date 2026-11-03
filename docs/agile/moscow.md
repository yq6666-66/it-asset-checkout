IT Asset Checkout is a custom equipment-loan scenario, not a course sample.

# MoSCoW

| Priority | Item | Reason |
|---|---|---|
| Must | Sign-in, the three registers, and the one-open-checkout rule | Without these the desk cannot show who holds a device. |
| Must | Seed data of at least five rows in each register | The relations have to be visible on a fresh run. |
| Must | Search and sort on Assets and Checkouts, and search on Staff | That is the depth needed to find a row during a demonstration. |
| Should | Staff sort, field validation messages, and the in-app Test page | They make the registers easier to check and they are already in the build. |
| Should | xUnit for the date, email, and open-checkout rules | A failing rule should be visible without clicking through the site. |
| Could | Selenium, JMeter, and the written test pack | They evidence the same behaviour for a review. They are not required to record a loan. |
| Won't for now | A presentation deck and a live Trello invite from this repository | Those need a person at the board and a later slide pass. The checklist is in `trello.md`. |
