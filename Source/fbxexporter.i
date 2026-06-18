// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%rename("%s") FbxExporter;
%rename("%s") FbxExporter::SetFileExportVersion(FbxString pVersion);
/*
 * Allow exporting in blocking mode.
 * TODO: support non-blocking if there's demand for it.
 *
 * Non-blocking mode opens up the possibility of crashes from multi-threaded
 * use of the same FbxManager, or from funny garbage collection business.
 */
%rename("%s") FbxExporter::Export(FbxDocument*);

/* Explicitly ignore it or else it pops up despite -fvirtual and default ignore. */
%ignore FbxExporter::Initialize(const char* pFileName, int pFileFormat=-1, FbxIOSettings * pIOSettings=NULL);

/* Described as obsolete but not deprecated. */
%ignore FbxExporter::SetResamplingRate;
%ignore FbxExporter::SetDefaultRenderResolution;

/* SetProgressCallback is implemented in fbxprogress.i */
%define_fbxprogress(FbxExporter);

/* GetCurrentWritableVersions returns a null-terminated list of strings. That
 * takes some handling. */
%ignore FbxExporter::GetCurrentWritableVersions;
%rename ("%s") FbxExporter::_getCurrentWritableVersionsLength;
%rename ("%s") FbxExporter::_getCurrentWritableVersionByIndex;
%extend FbxExporter {
  %csmethodmodifiers _getCurrentWritableVersionsLength "private";
  %csmethodmodifiers _getCurrentWritableVersionByIndex "private";
  int _getCurrentWritableVersionsLength () {
    auto versions = $self->GetCurrentWritableVersions();
    int i = 0;
    for( ; versions[i] != nullptr; ++i) { }
    return i;
  }
  const char *_getCurrentWritableVersionByIndex(int i) {
    return $self->GetCurrentWritableVersions()[i];
  }
  %proxycode %{
  public string[] GetCurrentWritableVersions() {
    var versions = new string[_getCurrentWritableVersionsLength()];
    for(int i = 0, n = versions.Length; i < n; ++i) {
      versions[i] = _getCurrentWritableVersionByIndex(i);
    }
    return versions;
  }
  %}
}

%rename("%s") FbxExporter::GetFileHeaderInfo;

/*
 * An FbxExporter keeps the output file open (an OS file handle) until the native
 * object is destroyed. The generic csdestruct_derived typemap installed by
 * weakpointerhandle(FbxExporter) leaves Destroy() commented out -- it only
 * releases the SWIG weak-pointer handle -- so the file stays locked until the
 * owning FbxManager is destroyed. That breaks any caller that disposes the
 * exporter (e.g. a `using` block) and then immediately touches the file.
 *
 * Re-enable the guarded Destroy-on-Dispose, mirroring FbxManager (fbxmanager.i).
 * The guard only destroys once the last live C# reference is released, so extra
 * proxies handed out elsewhere are not torn out from under callers still using
 * them. This must come after weakpointerhandles.i (included earlier) so it
 * overrides the generic typemap.
 */
%typemap(csdestruct_derived, methodname="Dispose", methodmodifiers="public") FbxExporter %{{
    if (swigCPtr.Handle != global::System.IntPtr.Zero
        && $imclassname.IsLastLiveWeakPointerReference(swigCPtr) != 0) { Destroy(); }
    base.Dispose();
  }%}

#ifndef SWIG_GENERATING_TYPEDEFS
// TODO: should we be more specific, test each function in turn for whether it can
// actually take null?
%apply FbxDocument * MAYBENULL { FbxDocument *pDocument };
#endif

%include "fbxsdk/fileio/fbxexporter.h"
