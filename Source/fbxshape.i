// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

// Unignore class
%rename("%s", %$isclass) FbxShape;
%rename("%s") FbxShape::SetAbsoluteMode;
%rename("%s") FbxShape::IsAbsoluteMode;

%include "fbxsdk/scene/geometry/fbxshape.h"