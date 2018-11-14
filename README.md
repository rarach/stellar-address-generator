# stellar-address-generator

Vanity account ID generator for the Stellar network written in C#.
Only prefixes are matched. Therefore they must always start with 'G'. No prefix validation is performed.

Inputs are only taken from App.config:
- threadCount: number of threads to run. It makes sense to set it to number of CPU cores.
- outputDir: full path to directory where output files will be created. Each running thread will produce one file.
- prefixes: comma-separated list of account ID prefixes to search for. Ex. GDDDDD,GBARBEQUE,GANIMEDE

Performance is not realy considered, i.e. it could probably be written way more efficient.

Special feature: put --blackhole as command line argument to generate "blackhole" public keys, i.e. addresses that
have unknown private keys. For instance, the prefix GAAAACEMETERYOFFORGOTTENHEROCOINS would generate address
GAAAACEMETERYOFFORGOTTENHEROCOINSXXXXXXXXXXXXXXXXXXXWYFK. The filler character ('X' in previous example) comes
from App.config.
