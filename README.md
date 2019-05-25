# Eventual Consistency

## Vordefinierte Ziele

### Kurzbeschreibung

AP-Datenverwaltung (im Sinne des [CAP Theorems](https://en.wikipedia.org/wiki/CAP_theorem)):
Mehrere Producer/Clients generieren 'Views' auf verschiedene Services.
Diese sollen miteinander kommunizieren und eventuell auf denselben Stand kommen.
Dabei wird die Kommunikation bewusst erschwert, indem Nachrichten zufällig verschluckt werden
oder länger zum Senden benötigen.

Siehe [Eventual Consistency](https://en.wikipedia.org/wiki/Eventual_consistency).

### Ziel/erwartetes Ergebnis

Mindestens 2 Services, die miteinander kommunizieren und ihre Daten (asynchron) synchronisieren wollen.
Diese werden von einem eigenen Simulator verwaltet und bestimmt.

Kommunikation zwischen den Services soll durch künstliche Komplikationen auf ihre Stabilität geprüft werden.

### Eingesetzte Technologien

- .NET core
- RabbitMQ
- ASP.NET

### Vorgehensweise





## Theorie

- Eventual Consistency
- Service-Syncronisation
- Caching



- Enventual Consistency darf keine Requests verlieren -> Advert-Money



## Komponenten



### Cheetah (Simulator)

- Methoden
- CLI Kommandos
- Simulator und Verbindungsstück



### Client

- Abstraktion von Gateway (sowohl Cheetah, als auch Gateway können diese Schnittstelle verwenden)
- Pro ViewService automatisch ein Client

### Gateway

- Rest-Service, um Client-Anfragen zu testen

### ViewService

- Kernstück
- Auflistung der Features

### Utility-Klassen



## Beispiel

- Schreibtischtest
- Veranschaulichung



## Ausführung



- Screenshots von Cheetah
- RabbitMQ-Managment Frontend
- DB-Files
- Terminal-Output



## Quellen

- [Why Computers Can't Count Sometimes](https://www.youtube.com/watch?v=RY_2gElt3SA)
- [Embracing eventual consistency in SoA networking](https://blog.envoyproxy.io/embracing-eventual-consistency-in-soa-networking-32a5ee5d443d)
- [Messaging as a programming model Part 2](https://eventuallyconsistent.net/2013/08/14/messaging-as-a-programming-model-part-2/)
- [Eventually consistent](https://eventuallyconsistent.net/tag/c/)
- [Eventual vs Strong Consistency in Distributed Databases](https://hackernoon.com/eventual-vs-strong-consistency-in-distributed-databases-282fdad37cf7)
