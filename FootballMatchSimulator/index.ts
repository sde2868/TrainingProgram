enum EventType {
    RUN = "RUN",
    PASS = "PASS",
    CELEBRATE = "CELEBRATE",
    SHOT = "SHOT",
    LONG_PASS = "LONG_PASS",
    TACKLE = "TACKLE",
    SAVE = "SAVE",
    GOAL = "GOAL"
};

type ScoreObject = {
    teamAScore: number;
    teamBScore: number;
};

type MatchStatistics = {
    goals: MatchEvent[];
    shots: MatchEvent[];
    saves: MatchEvent[];
    tackles: MatchEvent[];
    longPasses: MatchEvent[];
};

class Player {
    name: string;
    constructor(name: string) {
        this.name = name;
    }
    run(): EventType {
        return EventType.RUN;
    }
    pass(): EventType {
        return EventType.PASS;
    }
    celebrate(): EventType {
        return EventType.CELEBRATE;
    }
}

class Striker extends Player {
    constructor(name: string) {
        super(name);
    }
    shoot(): EventType {
        return EventType.SHOT;
    }
}

class Midfielder extends Player {
    constructor(name: string) {
        super(name);
    }
    longPass(): EventType {
        return EventType.LONG_PASS;
    }
}

class Defender extends Player {
    constructor(name: string) {
        super(name);
    }
    tackle(): EventType {
        return EventType.TACKLE;
    }
}

class GoalKeeper extends Player {
    constructor(name: string) {
        super(name);
    }
    save(): EventType {
        return EventType.SAVE;
    }
}

class Team {
    teamName: string;
    players: Player[];
    constructor(name: string) {
        this.teamName = name;
        this.players = new Array<Player>();
    }
    addPlayer(player: Player): void {
        this.players.push(player);
    }
    removePlayer(player: Player): boolean {
        const index = this.players.indexOf(player);
        if (index === -1) {
            return false;
        }
        this.players.splice(index, 1);
        return true;
    }
}

class MatchEvent {
    player: Player;
    team: Team;
    timeStamp: number;
    eventType: EventType;
    constructor(player: Player, team: Team, timeStamp: number, eventType: EventType) {
        this.player = player;
        this.team = team;
        this.timeStamp = timeStamp;
        this.eventType = eventType;
    }
}

class Match {
    matchNumber: number;
    teamA: Team;
    teamAScore: number;
    teamB: Team;
    teamBScore: number;
    events: MatchEvent[];
    commentary: Commentary;
    constructor(matchNumber: number, teamA: Team, teamB: Team, commentary: Commentary) {
        this.matchNumber = matchNumber;
        this.teamA = teamA;
        this.teamB = teamB;
        this.teamAScore = 0;
        this.teamBScore = 0;
        this.events = [];
        this.commentary = commentary;
    }
    recordPlayerAction(player: Player, team: Team, timeStamp: number, eventType: EventType): void {
        const event = new MatchEvent(player, team, timeStamp, eventType);
        this.recordEvent(event);
    }
    recordEvent(event: MatchEvent): void {
        this.events.push(event);
        const scoreObject: ScoreObject = {
            teamAScore: this.teamAScore,
            teamBScore: this.teamBScore
        }
        this.commentary.announce(event, scoreObject);
    }
    goalScored(player: Player, team: Team, timeStamp: number): void {
        if (team === this.teamA) {
            this.teamAScore++;
        }
        else if (team === this.teamB) {
            this.teamBScore++;
        }
        this.recordPlayerAction(player, team, timeStamp, EventType.GOAL);
    }
    getWinner(): Team | null {
        if (this.teamAScore > this.teamBScore) {
            return this.teamA;
        }
        else if (this.teamAScore < this.teamBScore) {
            return this.teamB;
        }
        else {
            return null;
        }
    }
    getEventsByType(eventType: EventType): MatchEvent[] {
        return this.events.filter(e => e.eventType === eventType);

    }
    getStatistics(): MatchStatistics {
        const matchStatistics: MatchStatistics = {
            goals: this.getEventsByType(EventType.GOAL),
            shots: this.getEventsByType(EventType.SHOT),
            saves: this.getEventsByType(EventType.SAVE),
            tackles: this.getEventsByType(EventType.TACKLE),
            longPasses: this.getEventsByType(EventType.LONG_PASS)
        }
        return matchStatistics;
    }
    displayMatchSummary(): void {
        const winner = this.getWinner();
        const stats = this.getStatistics();

        console.log("\n===== MATCH SUMMARY =====");
        console.log(`Match ${this.matchNumber}: ${this.teamA.teamName} ${this.teamAScore} - ${this.teamBScore} ${this.teamB.teamName}`);
        if (winner === null) {
            console.log("Result: Draw");
        } else {
            console.log(`Winner: ${winner.teamName}`);
        }
        
        console.log("=========================");
        console.log("\nStatistics:");

        const teamAGoalSummary = stats.goals.filter(goal => goal.team === this.teamA)
            .map(goal => `${goal.player.name} (${goal.timeStamp}')`)
            .join(", ");
        const teamBGoalSummary = stats.goals.filter(goal => goal.team === this.teamB)
            .map(goal => `${goal.player.name} (${goal.timeStamp}')`)
            .join(", ");

        console.log("\nGoals:");
        console.log(`${this.teamA.teamName}: ${teamAGoalSummary || "None"}`);
        console.log(`${this.teamB.teamName}: ${teamBGoalSummary || "None"}`);

        const teamAShotSummary = stats.shots.filter(shot => shot.team === this.teamA)
                   .map(shot => `${shot.player.name} (${shot.timeStamp}')`)
                   .join(", ");
        const teamBShotSummary = stats.shots.filter(shot => shot.team === this.teamB)
                   .map(shot => `${shot.player.name} (${shot.timeStamp}')`)
                   .join(", ");

        console.log("\nShots:");
        console.log(`${this.teamA.teamName}: ${teamAShotSummary || "None"}`);
        console.log(`${this.teamB.teamName}: ${teamBShotSummary || "None"}`);
        
        const teamASaveSummary = stats.saves.filter(save => save.team === this.teamA)
                   .map(save => `${save.player.name} (${save.timeStamp}')`)
                   .join(", ");
        const teamBSaveSummary = stats.saves.filter(save => save.team === this.teamB)
                   .map(save => `${save.player.name} (${save.timeStamp}')`)
                   .join(", ");

        console.log("\nSaves:");
        console.log(`${this.teamA.teamName}: ${teamASaveSummary || "None"}`);
        console.log(`${this.teamB.teamName}: ${teamBSaveSummary || "None"}`);
        
        const teamATackleSummary = stats.tackles.filter(tackle => tackle.team === this.teamA)
                   .map(tackle => `${tackle.player.name} (${tackle.timeStamp}')`)
                   .join(", ");
        const teamBTackleSummary = stats.tackles.filter(tackle => tackle.team === this.teamB)
                   .map(tackle => `${tackle.player.name} (${tackle.timeStamp}')`)
                   .join(", ");

        console.log("\nTackles:");
        console.log(`${this.teamA.teamName}: ${teamATackleSummary || "None"}`);
        console.log(`${this.teamB.teamName}: ${teamBTackleSummary || "None"}`);
        
        const teamALongPassSummary = stats.longPasses.filter(longPass => longPass.team === this.teamA)
                   .map(longPass => `${longPass.player.name} (${longPass.timeStamp}')`)
                   .join(", ");
        const teamBLongPassSummary = stats.longPasses.filter(longPass => longPass.team === this.teamB)
                   .map(longPass => `${longPass.player.name} (${longPass.timeStamp}')`)
                   .join(", ");

        console.log("\nLong Passes:");
        console.log(`${this.teamA.teamName}: ${teamALongPassSummary || "None"}`);
        console.log(`${this.teamB.teamName}: ${teamBLongPassSummary || "None"}`);
        
        console.log("=========================");
    }
}

interface Commentary {
    announce(event: MatchEvent, scoreObject: ScoreObject): void
}

class EnglishCommentary implements Commentary {
    announce(event: MatchEvent, scoreObject: ScoreObject): void {
        switch (event.eventType) {
            case EventType.GOAL:
                console.log(`${event.timeStamp}': ${event.player.name} scores for ${event.team.teamName}!
                    The score is now ${scoreObject.teamAScore}-${scoreObject.teamBScore}.`);
                break;
            case EventType.SHOT:
                console.log(`${event.timeStamp}': ${event.player.name} shoots...`);
                break;
            case EventType.SAVE:
                console.log(`${event.timeStamp}': Good save by ${event.player.name} for ${event.team.teamName}.`);
                break;
            case EventType.TACKLE:
                console.log(`${event.timeStamp}': Good tackle by ${event.player.name} for ${event.team.teamName}.`);
                break;
            default:
                console.log(`${event.timeStamp}': ${event.player.name} does ${event.eventType}.`);
                break;
        }
    }
}

const Messi = new Striker("Messi");
const TerStegen = new GoalKeeper("Ter Stegen");
const Barcelona = new Team("Barcelona");
Barcelona.addPlayer(Messi);
Barcelona.addPlayer(TerStegen);

const Ronaldo = new Striker("Ronaldo");
const Courtois = new GoalKeeper("Courtois");
const RealMadrid = new Team("Real Madrid");
RealMadrid.addPlayer(Ronaldo);
RealMadrid.addPlayer(Courtois);

const elClassico = new Match(1, Barcelona, RealMadrid, new EnglishCommentary());

Messi.shoot();
elClassico.recordPlayerAction(Messi, Barcelona, 21, Messi.shoot());
elClassico.goalScored(Messi, Barcelona, 21);

Ronaldo.shoot();
elClassico.recordPlayerAction(Ronaldo, RealMadrid, 44, Ronaldo.shoot());
TerStegen.save();
elClassico.recordPlayerAction(TerStegen, Barcelona, 44, TerStegen.save());

Ronaldo.shoot();
elClassico.recordPlayerAction(Ronaldo, RealMadrid, 67, Ronaldo.shoot());
elClassico.goalScored(Ronaldo, RealMadrid, 67);

elClassico.displayMatchSummary();