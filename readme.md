# tizenscan
A tool for finding tizen devices on your network, so you can sdb to the device easily

## usage
The default timeout is 500ms.
```bash
tizenscan [timeout in ms]
```

```bash
C:\Users\JohnDoe>tizenscan
Scanning network for tizen devices..
192.168.8.133
```
```bash
C:\Users\JohnDoe>tizenscan 1000
Scanning network for tizen devices..
192.168.8.133
```

## building the project
Publish the project to your arcitecture, and put the files in a place that your $path points to.
Enjoy!

## how it works
Scans the 255 available ips on your default networks, for a server on the sdb port 26101.