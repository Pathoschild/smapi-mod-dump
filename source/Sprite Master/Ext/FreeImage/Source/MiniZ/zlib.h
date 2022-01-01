#pragma once

#include "miniz.h"

#define inflateReset2(a, b) mz_inflateReset(a)
#define inflateValidate(...) (Z_OK)
