// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%rename("%s") FbxImporter;

/*
 * Allow importing in blocking mode.
 * TODO: support non-blocking if there's demand for it.
 *
 * Non-blocking mode opens up the possibility of crashes from multi-threaded
 * use of the same FbxManager, or from funny garbage collection business.
 */
%rename("%s") FbxImporter::Import(FbxDocument*);

/* SetProgressCallback is implemented in fbxprogress.i */
%define_fbxprogress(FbxImporter);

/* Explicitly ignore it or else it pops up despite -fvirtual and default ignore. */
%ignore FbxImporter::Initialize(const char* pFileName, int pFileFormat=-1, FbxIOSettings * pIOSettings=NULL);

%ignore SetPassword;
%rename("%s") FbxImporter::IsFBX;
%rename("%s") FbxImporter::GetFileVersion;
%rename("%s") FbxImporter::GetAnimStackCount;
%rename("%s") FbxImporter::GetActiveAnimStackName;
%rename("%s") FbxImporter::GetFileHeaderInfo;
%rename("%s") FbxImporter::GetTakeInfo;

/*
 * An FbxImporter keeps the input file open (an OS file handle) until the native
 * object is destroyed. The generic csdestruct_derived typemap installed by
 * weakpointerhandle(FbxImporter) leaves Destroy() commented out -- it only
 * releases the SWIG weak-pointer handle -- so the file stays locked until the
 * owning FbxManager is destroyed. That breaks any caller that disposes the
 * importer (e.g. a `using` block) and then immediately touches the file.
 *
 * Re-enable the guarded Destroy-on-Dispose, mirroring FbxManager (fbxmanager.i).
 * The guard only destroys once the last live C# reference is released, so extra
 * proxies handed out elsewhere are not torn out from under callers still using
 * them. This must come after weakpointerhandles.i (included earlier) so it
 * overrides the generic typemap.
 */
%typemap(csdestruct_derived, methodname="Dispose", methodmodifiers="public") FbxImporter %{{
    if (swigCPtr.Handle != global::System.IntPtr.Zero
        && $imclassname.IsLastLiveWeakPointerReference(swigCPtr) != 0) { Destroy(); }
    base.Dispose();
  }%}

%include "fbxsdk/fileio/fbximporter.h"
