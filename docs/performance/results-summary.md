IT Asset Checkout is a custom equipment-loan scenario, not a course sample.

# JMeter run on 2026-11-03

The GUI was not used. Apache JMeter 5.6.3 ran from the command line against the local site on `http://127.0.0.1:5080`. 演示账号见 `.env.example`. Pass the password only on the command line with `-JdemoPassword`. Demo environment only.

## Command

```bash
jmeter -n -t docs/performance/it-asset-checkout.jmx -JdemoPassword="$DEMO_ADMIN_PASSWORD" -l docs/performance/results.jtl -e -o /tmp/jmeter-report
```

The plan uses 20 threads, a 5 second ramp-up, and a 60 second scheduler. Each iteration clears cookies, loads the login page, posts the demo sign-in, and then requests `/Assets`. The sign-in assertion looks for `IT-10482`. The list assertion looks for `Dell Latitude 5440`.

## What the run showed

CLI summary: 12,183 samples in 60 seconds, about 203 per second, 0 errors.

`statistics.json` counts redirect sub-samples as well (20,311 rows). Error rate 0 percent. For the total row, the report's 95th percentile (`pct2ResTime`) is 239 ms and the maximum is 520 ms. The slowest label, `POST login`, has a 95th percentile of 309 ms.

Both targets are met on this machine: 95th percentile under 1 second, and error rate under 1 percent. This is one local Kestrel process, not a deployed server.

The dashboard screenshot is `jmeter-dashboard.png`. It shows a 100 percent pass pie for the same report. Regenerate the HTML folder with the `-e -o` switch above; those generated files are not all stored in git. The raw log is `results.jtl` and the aggregate is `statistics.json`.
