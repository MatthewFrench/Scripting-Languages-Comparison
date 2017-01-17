# Scripting-Languages-Comparison
Compares CSScript, Roslyn and Lua in C#

------

Current results:

Preparing 1000 Lua Instances Elapsed(MS) = 485 - FPS: 2.06185567010309

Retrieving a value from a math function parsed at runtime 1000 times Elapsed(MS) = 11 - FPS: 90.9090909090909

Setting a global value 1000 times Elapsed(MS) = 6 - FPS: 166.666666666667

Retrieving a global value 1000 times Elapsed(MS) = 1 - FPS: 1000

Setting a function 1000 times Elapsed(MS) = 34 - FPS: 29.4117647058824

Calling a function 1000 times Elapsed(MS) = 143 - FPS: 6.99300699300699

Calling a single heavy lua function returned value: True, Elapsed(MS) = 1249 - FPS: 0.800640512409928

Total NLUA Elapsed(MS) = 1952 - FPS: 0.512295081967213

------

Separate instances of a CSScript context do not exist

Retrieving a value from a math function parsed at runtime 1000 times Elapsed(MS) = 1554 - FPS: 0.643500643500644

Setting a function 1000 times Elapsed(MS) = 1413 - FPS: 0.707714083510262

Calling a function 1000 times Elapsed(MS) = 38 - FPS: 26.3157894736842

Calling a single heavy C# function returned value: True, Elapsed(MS) = 542 - FPS: 1.8450184501845

Total CSScript Elapsed(MS) = 3553 - FPS: 0.281452293836195

------

Separate instances of a Roslyn context do not exist

Retrieving a value from a math function parsed at runtime 1000 times Elapsed(MS) = 13616 - FPS: 0.0734430082256169

Setting a function 1000 times Elapsed(MS) = 82899 - FPS: 0.0120628716872339

Calling a function 1000 times Elapsed(MS) = 50 - FPS: 20

Calling a single heavy C# function returned value: True, Elapsed(MS) = 666 - FPS: 1.5015015015015

Total Roslyn Elapsed(MS) = 97273 - FPS: 0.0102803450083785
