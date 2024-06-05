grammar MyGrammar;

Pr : PDs ;

PDs : PD PDs
    | PD
    ;

PD : E
   | C
   ;

E : 'effect' '{' EPs '}' ;

EPs : EP ',' EPs
    | EP
    ;

EP : N
   | P
   | Ac
   ;

N : 'Name' ':' STRING ;

P : 'Params' ':' '{' Ds '}'
  | 'e'
  ;

Ds : D ',' Ds
   | D
   ;

D : ID ':' TD ;

Ac : 'Action' ':' '(' 'targets' ',' 'context' ')' '=>' '{' IB '}' ;

C : 'card' '{' CPs '}' ;

CPs : CP ',' CPs
    | CP
    ;

CP : N
   | T
   | F
   | PO
   | RB
   | AB
   ;

T : 'Type' ':' STRING ;

F : 'Faction' ':' STRING ;

PO : 'Power' ':' ExN
   | 'e'
   ;

RB : '[' STRING ',' STRINGS ']' ;

AB : 'OnActivation' ':' '[' EList ']' ;

EList : CE ',' EList
      | CE
      ;

CE : '{' ECPs '}' ;

ECPs : ECP ',' ECPs
     | ECP
     ;

CED : 'Effect' ':' EB ;

S : 'Selector' ':' '{' SPs '}' ;

PAc : 'PostAction' ':' '{' PAcPs '}'
    | 'e'
    ;

EB : '{' EBPs '}'
   | STRING
   ;

EBPs : EBP ',' EBPs
     | EBP
     ;

SP : SO
   | SI
   | PR
   ;

SO : 'Source' ':' STRING ;

SI : 'Single' ':' ExB
   | 'e'
   ;

PR : 'Predicate' ':' ExPr ;

PAcPs : PAcP ',' PAcPs
      | PAcP
      ;

// Tokens
STRING : '"' .*? '"' ;
ID : [a-zA-Z_] [a-zA-Z_0-9]* ;
TD : ID ;
IB : ID ;
ExB : ID ;
ExPr : ID ;
ExN : ID ;
