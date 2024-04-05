# Price-Rack:
Price-Rack is a simple microservice to give the value of financial instruments at a given point.
As of now it supports only BTC/USD pair, but it can be extended to support other pairs as well.

# Working:
Price Rack fetches the prices from two sources as of now
1) https://www.bitstamp.net/api/#ohlc_data
2) https://docs.bitfinex.com/reference/rest-public-candles

aggregate them, then return the value to the client. The values are stored in the storage as well, which are used when user requests for the same values later. 

# Technologies:
.Net 6
 AND SqlLite

# Dependencies:
.Net  6
 

# How to use:
1) Install .Net 6 SDK
2) Install Visual Studio 2022
3) Open the solution file and build it
4) Select PriceRack(API Project) as start-up project
5) Run the program 
6) If there are certificates launched accept them
7) In case you face the issue: "Your connection is not private" resolve it this way.
 a) Open chrome
 b) In the address bar type: chrome://flags
 c) Now type "localhost" in the search bar
 d) Enable the option "Allow invalid certificates for resources loaded from localhost" 
 e) Relaunch chrome
 8) Relaunch the project and test

#How to test:
On project launch, you should see two endpoints:

![image](https://user-images.githubusercontent.com/43916885/235371880-5da7949d-5015-4249-8c86-6ee8984a2284.png)
1) Price
Curl:
  curl -X 'GET' \
  'https://localhost:7230/Prices/price?time=2023-04-30T19%3A00%3A00Z' \
  -H 'accept: text/plain'

You can use the time in this format: 2023-04-30T19:00:00

Expected response: 
![image](https://user-images.githubusercontent.com/43916885/235372108-0f7f7046-8322-4672-acb7-b3d32a6d3660.png)


2) History
Curl:
curl -X 'GET' \
  'https://localhost:7230/Prices/history?start=2023-03-30T19%3A00%3A00&end=2023-04-30T19%3A00%3A00' \
  -H 'accept: text/plain'

You can use the time in this format: 2023-04-30T19:00:00

Expected response: 
![image](https://user-images.githubusercontent.com/43916885/235372121-351e15a7-d091-4bb8-96b6-633a3d598536.png)

