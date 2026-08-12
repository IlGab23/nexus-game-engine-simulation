# nexus-game-engine-simulation
"Nexus Game Engine Simulation" è un robusto backend in .NET 10 per RPG multiplayer. Progettato con Clean Architecture e CQRS, gestisce ad alta concorrenza identità, inventari ed economie transazionali di gioco. Integra processi asincroni, cache e gRPC per eventi del mondo e classifiche in tempo reale, offrendo massime prestazioni.


<div align="center">
  <h1>🚀 Nexus Game Engine Simulation</h1>
  <p><i>Un robusto backend enterprise in .NET 10 progettato per orchestrare le meccaniche core, l'economia transazionale e la concorrenza di un RPG multiplayer.</i></p>
</div>

## 📖 Descrizione del Progetto

**Nexus Game Engine Simulation** è l'infrastruttura backend per un videogioco multiplayer su media scala. Sviluppato utilizzando le tecnologie più recenti dell'ecosistema Microsoft (.NET 10 e C# 14), il progetto incapsula la complessa logica di business di un gioco di ruolo all'interno di una solida **Clean Architecture**.

L'applicazione non si limita a gestire semplici operazioni di lettura e scrittura, ma funge da vero e proprio motore per le meccaniche di gioco: orchestra l'evoluzione dei giocatori, gestisce scambi economici complessi prevenendo frodi, e aggiorna costantemente lo stato del mondo virtuale. Grazie all'implementazione del pattern **CQRS**, all'uso del caching ibrido e alla comunicazione RPC, il sistema è intrinsecamente progettato per resistere ad alta concorrenza e garantire altissime prestazioni.

## ✨ Funzionalità Principali

* 🛡️ **Identità e Sicurezza Avanzata:** Gestione sicura dei giocatori con hashing delle password tramite Argon2id e un flusso di autenticazione basato su JWT. Implementa meccanismi enterprise come *Refresh Token Rotation* e *Reuse Detection* protetti da cookie sicuri (HttpOnly/Secure), assieme a Rate Limiting nativo e autorizzazione granulare basata su Policy.
* 🎒 **Inventario ed Economia Thread-Safe:** Un sistema transazionale che impedisce categoricamente attacchi di duplicazione (duping) e race condition. Ogni operazione critica sull'inventario sfrutta lock asincroni (`SemaphoreSlim`) per una gestione concorrente impeccabile, mantenendo la persistenza tramite Entity Framework Core e `IApplicationDbContext`.
* 🌍 **Mondo Persistente e Job Asincroni:** Integrazione profonda con **Quartz.NET** e un database isolato gestito da DbUp per eseguire job pianificati (es. reset giornalieri) e processare in background code di eventi asincroni, sgravando completamente il flusso principale di richiesta/risposta.
* ⚡ **Prestazioni ed RPC (gRPC):** Risposte a bassissima latenza per leaderboard e cataloghi degli oggetti grazie all'implementazione di `HybridCache`. Le comunicazioni ad altissima frequenza (come l'aggiornamento costante della posizione o della salute da parte del server di gioco) sono demandate a endpoint **gRPC** dedicati, protetti a loro volta da JWT.
* 🏗️ **Architettura e Qualità del Codice:** Sviluppo basato sul *Result Pattern* per un controllo di flusso privo di eccezioni, ed ecosistema strutturato su MediatR con pipeline di validazione (*Fluent Validation*) e logging. L'implementazione di `TimeProvider` assicura la totale testabilità dei cooldown delle azioni in-game.

---
*Progetto interamente containerizzato via Docker per uno sviluppo, rilascio ed esecuzione fluidi.*

## 🎯 Scopo del Progetto
Questa applicazione nasce come un esercizio pratico personale, ideato per allenare, approfondire e perfezionare le mie competenze nello sviluppo di architetture backend complesse utilizzando .NET Web API. È a tutti gli effetti una "palestra" per sperimentare design pattern avanzati e best practice di livello enterprise in un contesto realistico.

<br>

<div align="center">
  <h3>🚧 Work in progress 🚧</h3>
  <p><i>Il progetto è attualmente in fase di sviluppo. Struttura e funzionalità sono in continua implementazione ed evoluzione.</i></p>
</div>