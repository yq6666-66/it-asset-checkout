IT Asset Checkout is a custom equipment-loan scenario, not a course sample.

# User stories

| ID | Story | Done when |
|---|---|---|
| US-01 | As a desk admin, I want to sign in so that only a known user can change equipment records. | The demo account can sign in and the header shows that username. |
| US-02 | As a desk admin, I want a home page and navigation so that I can reach Assets, Staff, Checkouts, and the Test page. | Those links are visible, and the home page shows the logo, colour, and banner. |
| US-03 | As a desk admin, I want to create, read, update, and delete assets so that the catalogue stays current. | Tag number, name, category, status, purchase date, and value can be saved and validated. |
| US-04 | As a desk admin, I want to search and sort assets so that I can find a device by tag, name, category, or status. | The Assets list filters and reorders from the search form. |
| US-05 | As a desk admin, I want to keep staff records so that a loan names a real person. | Staff code, full name, email, and department support CRUD, search, and sort. |
| US-06 | As a desk admin, I want to record checkouts so that I can see who holds an asset and when it is due. | A checkout stores the asset, the staff member, checkout date, due date, optional return date, and a note. |
| US-07 | As a desk admin, I want the system to refuse a second open loan of the same asset so that one device is not promised twice. | Saving another checkout with an empty return date for that asset shows an error. |
| US-08 | As a reviewer, I want a Test page and automated checks so that the rules above can be repeated. | The Test page lists UT-01 through UT-05, and `dotnet test` runs those methods. |
