# tizenscan
A tool for finding tizen devices on your network, so you can sdb to the device easily

As of right now, the ip of the tizen device must start with 192.168.8

## usage
```bash
C:\Users\JohnDoe>tizenscan
Tizen devices:
192.168.8.133
```

## building the project
Publish the project to your arcitecture, and put the files in a place that your $path points to.
Enjoy!

## how it works
the tools uses `arp -a` to find accessable endpoints and their mac addresses. 
Then it performs a lookup with api.macaddress.io, and checks if the mac address belongs to samsung
if it does, the ip is printed to the terminal