# CustomMoq
Custom implementation of Moq library.  
Implemented two approaches:
 - Runtime compilation of code using `CSharpCompilation`.  
   Whole assembly compiled from scratch, therefore it very slow.
 - Runtime compilation of code using ![Castle.Core](https://github.com/castleproject/Core/tree/master) library.  
   It uses `IL` code generation under the hood and it performs really fast.  
   `Moq` and `NSubstitute` use this library.

