
open canopy
open runner
open configuration

start firefox
compareTimeout <- 1.0

"Look for name" &&& fun _ ->
    url "http://localhost:18572/"
    ".name" != ""

run()

quit()

