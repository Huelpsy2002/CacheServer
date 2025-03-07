# Cache Server

A simple cache server that intercepts client requests and caches responses from the actual server. If a cached response exists, it is served directly from the cache instead of forwarding the request to the actual server. This helps reduce load on the actual server and improves response time.

## Features
- Caches responses to reduce redundant requests to the actual server.
- Supports command-line arguments for configuration.
- Automatically forwards requests when cache is unavailable.
- Simple and lightweight, implemented in C#.



## Installation

1 - git clone https://github.com/Huelpsy2002/CacheServer.git
2- cd CasheServer
3- dotnet build 
4 - dotnet run --port 8080 --url https://jsonplaceholder.typicode.com


## Usage
Run the cache server with the following command:
CacheServer --port <port> --url <url>

## Options
--port, -p      : Port number to run the server
--url, -u       : URL of the actual server
--help, -h      : Display this help information

## Example
CacheServer --port 8080 --url https://jsonplaceholder.typicode.com



## Contributing
Contributions are welcome! Feel free to submit a pull request or open an issue.

