# Eventual Consistency

## Vordefinierte Ziele

### Kurzbeschreibung

AP-Datenverwaltung (im Sinne des [CAP Theorems](https://en.wikipedia.org/wiki/CAP_theorem)): Mehrere Producer/Clients generieren 'Views' auf verschiedene Services.
Diese sollen miteinander kommunizieren und eventuell auf denselben Stand kommen. Dabei wird die Kommunikation bewusst erschwert, indem Nachrichten zufällig verschluckt werden oder länger zum Senden benötigen.

Siehe [Eventual Consistency](https://en.wikipedia.org/wiki/Eventual_consistency).

### Ziel/erwartetes Ergebnis

Mindestens 2 Services, die miteinander kommunizieren und ihre Daten (asynchron) synchronisieren wollen. Diese werden von einem eigenen Simulator verwaltet und bestimmt.

Kommunikation zwischen den Services soll durch künstliche Komplikationen auf ihre Stabilität geprüft werden.

### Eingesetzte Technologien

- .NET
- RabbitMQ

### Vorgehensweise



