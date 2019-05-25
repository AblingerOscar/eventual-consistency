# Eventual Consistency

## Vordefinierte Ziele

### Kurzbeschreibung

Es handelt sich um eine Deme einer AP-Datenverwaltung (im Sinne des [CAP Theorems](https://en.wikipedia.org/wiki/CAP_theorem)):
Mehrere Clients generieren 'Views' (vergleiche Views mit YouTube oder Likes in Sozialen Medien) auf verschiedene Services. Diese liegen bewusst getrennt vor.
Services sollen so miteinander kommunizieren, dass diese zumindest nach einer gewissen Zeit auf denselben Stand kommen.

Siehe [Eventual Consistency](https://en.wikipedia.org/wiki/Eventual_consistency).

### Ziel/erwartetes Ergebnis

Mindestens zwei Services, die miteinander kommunizieren und ihre Daten (asynchron) synchronisieren wollen.
Diese werden von einem eigenen Simulator verwaltet und bestimmt.

Kommunikation zwischen den Services soll durch künstliche Komplikationen auf ihre Stabilität geprüft werden.

### Eingesetzte Technologien

- .NET Core 2.1
- RabbitMQ
- ASP.NET
- NuGet Package: RabbitMQ.Client 5.1.0

### Vorgehensweise

Im ersten Schritt berieten wir uns ausführlicher darüber, wie wir die Applikation aufbauen wollen. Das Architektur-Design der einzelnen Komponenten wurde mehrmals überarbeitet,  um verschiedene Aspekte des Prinzips anwenden zu können. Schwierig war dabei die Entscheidung, wie viel wir über einen Rest-Client steuern möchten und wie viel über einen dezidierten Simulator laufen soll. Am Ende war das ASP.NET-Rest-Modul zuerst funktionierend, weshalb erste Tests hier durchgeführt wurden.

Je weiter die Implementierung des Simulators fortgeschritten war, desto mehr wurde dieser in der Entwicklung  verwendet, vor allem weil er bessere Konsolen-Outputs liefert und schneller und angenehmer verwendet werden kann.

Nachdem die Interfaces einigermaßen fixiert waren, konnte die Arbeit relativ getrennt aufgeteilt und am Ende die Applikation überraschend gut  zusammengeführt werden.

## Theorie

Zahlen addieren sollte für einen Computer grundsätzlich kein Problem darstellen. Wenn das aber verteilt und skalierbar durchgeführt werden muss, kommt man auf einige interessante Probleme oder Nebeneffekte, die hier in kurz dargestellt werden:

- Race conditions
    - Zwei Clients können zu gleichen Zeit eine Abfrage von Daten machen. Durch die interne Request-Reihenfolge könnte ein Update am Server zwischen den Abfragen die Ergebnisse beeinflussen. Man kann kaum festlegen, welcher Client zuerst beliefert wird.
    - Durch Balance-Loaders können Clients zu verschiedenen Servern (hier: Services) weitergeleitet werden, die unterschiedliche Ergebnisse liefern können.

- Caching
    - Wenn es mehrere Caching-Instanzen gibt, können Clients verschiedene Ergebnisse bekommen, je nachdem, wo sie hingeleitet werden.
- Eventual  Consistency
    - Man nimmt in Kauf, dass Server nicht immer absolut aktuelle Daten liefern. Sie sollen in einem Moment mit einer Abweichung stimmen und erst nach einer gewissen Zeitspanne zum korrekten Ergebnis kommen.

In diesem Projekt behandeln wir vor allem Eventual Consistency, die anderen Themen spielen aber trotzdem eine Rolle.



### AP - Verfügbarkeit und Partitionstoleranz 

Die Verfügbarkeit ist extrem hoch, ebenso Toleranz gegenüber dem Ausfall einzelner Server. Allerdings ist die Konsistenz nicht immer sofort gegeben: es kann durchaus eine längere Zeit dauern, bis alle Server Updates erhalten und damit von allen Clients gesehen wird. (Stichwort: horizontale Skalierunng)



Beispiele für Web-Anwendungen, die nicht auf strenge Konsistenz angewiesen sind, wären Social-Media-Sites wie Twitter oder Facebook; wenn einzelne Nachrichten nicht bei allen Nutzern  gleichzeitig eintreffen, ist dadurch die prinzipielle Funktion des Dienstes nicht beeinträchtigt. 

Allerdings darf selbst bei diesen Applikationen keine Anfrage verloren gehen, wenn man beispielsweise and YouTube  denkt, wo Werbeeinnahmen aufgrund von der Anzahl ann Videoaufrufen generiert werden.



## Komponenten

### Gateway

Das Gateway ist eine ASP.Net-Anwendung, die eine Rest-Schnittstelle zu den ViewServices darstellt. Hier können Views ausgelesen und neue hinzugefügt werden.

Die Kommunikation mit den Services erfolgt über RabbitMQ mit dem Request/Reply-Pattern (Implementierung in `RPCGatewayClient`).



Die definierten Routen sind folgende:

- `GET api/{serviceUid}/views`
- `POST api/{serviceUid}/add-view`
- `POST api/{serviceUid}/add-views/{number}`

Wie man hier erkennen kann, wird eine Anfrage immer an genau ein vordefiniertes  Service geschickt. In der Realität würde ein Load-Balancer eine Anfrage entgegennehmen und diese an einen Service weiterleiten (z.B.: an einen unausgelasteten oder jenen in der Nähe des Requests).



#### Screenshots

![](doc/img/gateway-01.png)

![](doc/img/gateway-02.png)



#### Herausforderungen

- SSL-Konfiguration und Port-Blocking
- Gemeinsames Starten von Simulator und ASP.NET



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
