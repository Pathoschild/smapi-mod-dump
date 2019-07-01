[← back to readme](readme.md)

# Release notes
## 1.0.1
* Fixed version number.

## 1.0
* All code is now async for performance.
* Moved from LiteDb to Azure Table Storage.
* Changed mail endpoint paths, names and parameters to be more clear.
* Creating mail on the server has changed verbs from POST to PUT.
* Added a `/api/mail/count` endpoint to check how much mail is on the server.
* Added a `/api/health` endpoint that just returns true.

## Before 1.0
* Intial version of the API (no release notes available).
