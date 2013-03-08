=====
RightScale.LatencyAndLoad ASP.net application
=====

This application is dead simple--in order to test the performance of a system without actualy nailing the back-end, I put together a really rudimentary app that returns a variable sized payload (centered around an average) and simulates network traffic and request latency.  All the while, it manages to write out a log that you can check to ensure that your actual latency #'s within the IIS/ASP.net stack are what you expected them to be.  The response itself is just a random # of bytes as defined by the average, min and max and optional "big" and "small" reqests by percentage.  Likewise, the app will generate a target latency in MS and if the app generates a response faster than that latency that's calculated, the web server will sleep until the latency period has elapsed before returning the response object to the client.

Within the web.config there are 12 keys you care about (in this order)

For Payload:
  * `avgPayload` - in bytes, the average size of a response back to the client
  * `stDevPayload` - in bytes, the standard deviation of your payloads (this will be important in a sec)
  * `minPayload` - in bytes, the smallest size your response object will be
  * `maxPayload` - in bytes, the largest size your response object will be
  * `pctPayloadSmall` - the percent of requests that will have a response size that is between the minPayload and (avgPayload - stDevPayload)
  * `pctPayloadBig` - the percent of requests that will have a response size that is between (avgPayload + stDevPayload) and the maxPayload defined

For Latency:
  * `avgLatency` - in milliseconds, the average amount of time IIS takes to process and respond to a request
  * `stDevLatency` - in milliseconds, the standard deviation of your latency 
  * `minLatency` - in milliseconds, the minimum amount time that the app server should take to respond with the data stream
  * `maxLatency` - in milliseconds, the maximum amount of time that the app server should take to responsd with the data stream
  * `pctLatencyLow` - the percent of requests that will have an app server latency that is between minLatency and (avgLatency - stDevLatency)
  * `pctLatencyHigh` - the percent of requests that will have an app server latency that is between (avgLatency + stDevPayload) and the maxLatency defined
  
I'll spend more time documenting it as needed, but this should get you a good first step into understanding how it operates.

P