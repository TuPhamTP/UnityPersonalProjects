mergeInto(LibraryManager.library, {
  DebugPictureJS: function(base64ImagePtr) {
        var base64Image = UTF8ToString(base64ImagePtr);
        DebugPicture(base64Image);
    },
  OpenWebsite: function() {
        NativeOpenWebsite();
    },
  GetDeviceType: function() {
        return NativeCheckDevice()? 1: 0;
    },
});