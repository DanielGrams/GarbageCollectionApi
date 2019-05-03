# GarbageCollectionApi

[![Build Status](https://travis-ci.com/DanielGrams/GarbageCollectionApi.svg?branch=master)](https://travis-ci.com/DanielGrams/GarbageCollectionApi)

API for garbage collection in the district of Goslar.

TODO:

- Logging beim Parsen erweitern
- Fehler werfen, wenn komische Werte gefunden werden
- Mail schicken, wenn Fehler? Oder kann man das über Azure App Service machen.
- Tests komplettieren
- Error Handling <https://code-maze.com/global-error-handling-aspnetcore/>
- Options/Configuration
- Kalenderjahr dynamisch anpassen: Man kann das Datum aus dem Dropdown auslesen. (Wenn man auch die Categories einliest)
- Events einschränkbar über Cagegories: api/town/{id}/streets/{streetId}/events?categories=1,4,7
- /api/status mit letztem stamp und letztem kwb-abruf (kann man für cronjob nehmen?)