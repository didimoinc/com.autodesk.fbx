// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME

// Unignore class
%rename("%s") FbxManager;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxManager::Create; 
%rename("%s") FbxManager::Destroy; 
%rename("%s") FbxManager::SetIOSettings;
%rename("%s") FbxManager::GetIOSettings;
%rename("%s") FbxManager::GetVersion;
%rename("%s") FbxManager::GetFileFormatVersion;
%rename("%s") FbxManager::GetIOPluginRegistry;

#endif

%nodefaultctor FbxManager;                      // Disable the default constructor for class FbxManager.

%apply int & OUTPUT { int & pMajor };
%apply int & OUTPUT { int & pMinor };
%apply int & OUTPUT { int & pRevision };

/*
 * FbxManager is the root owner of every object created under it. Unlike other
 * Fbx objects -- which are owned by their manager/scene and must NOT be
 * destroyed when a C# proxy is disposed (see FbxSharpObjectLifetime.i) --
 * disposing the manager is the explicit teardown point. So destroy the native
 * manager (which frees every object it owns) on Dispose, but only once the
 * last live C# reference to it is released: GetFbxManager() and the importer
 * can hand out extra proxies, and tearing the manager down while any of those
 * are still in use would leave them dangling.
 *
 * This overrides the csdestruct typemap that weakpointerhandle(FbxManager)
 * installs in weakpointerhandles.i (included earlier); it is otherwise
 * identical to the generic one, only re-enabling the guarded Destroy().
 */
%typemap(csdestruct, methodname="Dispose", methodmodifiers="public") FbxManager %{{
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }
  ~$csclassname() {
    Dispose(false);
  }
  protected void Dispose(bool disposing) {
    if (swigCPtr.Handle != global::System.IntPtr.Zero) {
      if (disposing) {
        if ($imclassname.IsLastLiveWeakPointerReference(swigCPtr) != 0) { Destroy(); }

      lock(this) {
        $imclassname.ReleaseWeakPointerHandle(swigCPtr);
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }}
  }%}

%include "fbxsdk/core/fbxmanager.h"
