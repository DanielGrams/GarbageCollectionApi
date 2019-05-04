# GarbageCollectionApi

[![Build Status](https://travis-ci.com/DanielGrams/GarbageCollectionApi.svg?branch=master)](https://travis-ci.com/DanielGrams/GarbageCollectionApi)

API for garbage collection in the district of Goslar.

TODO:

- /api/status mit letztem stamp und letztem kwb-abruf (kann man für cronjob nehmen?)
- Cronjob und dann einmal die Woche
- kompletter durchlauf
- Error Handling <https://code-maze.com/global-error-handling-aspnetcore/>
- Kalenderjahr dynamisch anpassen: Man kann das Datum aus dem Dropdown auslesen. (Wenn man auch die Categories einliest)
- Events einschränkbar über Categories: api/town/{id}/streets/{streetId}/events?categories=1,4,7