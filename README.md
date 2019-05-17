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



## Quellen

- [Why Computers Can't Count Sometimes](https://www.youtube.com/watch?v=RY_2gElt3SA)
- [Embracing eventual consistency in SoA networking](https://blog.envoyproxy.io/embracing-eventual-consistency-in-soa-networking-32a5ee5d443d)
- [[Messaging as a programming model Part 2](https://eventuallyconsistent.net/2013/08/14/messaging-as-a-programming-model-part-2/)](https://eventuallyconsistent.net/tag/c/)
- [Eventual vs Strong Consistency in Distributed Databases](https://hackernoon.com/eventual-vs-strong-consistency-in-distributed-databases-282fdad37cf7)