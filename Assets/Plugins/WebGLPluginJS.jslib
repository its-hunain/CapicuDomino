// Creating functions for the Unity
mergeInto(LibraryManager.library, {

   // Function with the text param
   PassTextParam: function (text) {
      // Convert bytes to the text
      var convertedText = Pointer_stringify(text);

      // Show a message as an alert
	window.parent.postMessage(
          { fromDominos: true, data: convertedText },'*'
        	);
   }
});