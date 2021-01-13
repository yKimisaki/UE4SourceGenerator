# UE4SourceGenerator

Simple and fast a pair of .h and .cpp files generator from project domain or standard UE4 base types (like UObject, AActor...) templates. 
You can share template files using version control for each project.

## Usage

![image](https://user-images.githubusercontent.com/1702680/104420505-4f25f680-55bd-11eb-9697-13bf14238c01.png)

1. Select output directory. A pair files will be generated on this directory.
2. If you select output directory, this tool search .uproject file from related directories, and show PROJECT_API above "Type Name" text box.
3. "Type Name" must contains UE4 style prefix, like UHogeObject or AFugaActor.
4. Select "Base Type" and click "Generate" button. 

## How to add project domain templates

For example, think about the case that create a new class inherited UMyObject class. (UMyObject is project domain type.)
At first, you have to create "SourceGeneratorTamplates" directory at same hierarchy as .uproject file.
Next, create UMyUObject.h.txt like following.

```
#pragma once

#include "CoreMinimal.h"
#include "Project/CustomTypes/MyUObject.h"
#include "{FileName}.generated.h"

UCLASS()
class {PROJECT_API} {TypeName} : public UMyUObject
{
	GENERATED_BODY()
};
```

This tool replace the specific string.

| From  | To |
| ------------- | ------------- |
| {PROJECT_API}  | UPPERCASE_API from .uproject file name |
| {TypeName}  | Generated type name with prefix. |
| {FileName}  | Generated file name without prefix. |

.h.txt template file is required.
.cpp.txt is not required, but if you need the definition for source file, you can define .cpp.txt template that is same name of .h.txt.
If you haven't define .cpp template, this tool generate a simple .cpp file.

```
#include "{FileName}.h"
```
