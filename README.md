Database Architecture

Organization
     │
     ├── Users / Roles
     │
     └── Tickets
           │
           ├── Category
           ├── Priority
           ├── Status
           ├── Assignment History
           ├── Comments
           ├── Event History
           │
           └── SLA
                 │
                 └── SLA Policy

Tickets
   │
   └── Outbox
         │
         └── RabbitMQ
                │
                └── Processed Events


                          organizations    
             ┌───────────┼──────┼─────┼─────────────┐
             │           │            │             │
             ▼           ▼            ▼             ▼
           users      categories    priorities    statuses
             │           │            │             │
             ▼           │            │             │
   organization_users    │            │             │
             │           │            │             │
             └───────────┴──────┬─────┴─────────────┘
                                │
                                ▼
                            tickets
                         /    |    \
                        /     |     \
                        ▼     ▼      ▼
                    comments events assignments
                                |
                                ▼
                            ticket_sla
                                |
                                ▼
                            sla_policies


                            tickets
                                │
                                ▼
                        outbox_messages
                                │
                                ▼
                            RabbitMQ
                                │
                                ▼
                        processed_events

Processed_Events Save from duplicates

                RabbitMQ
                    ↓
                Event #ABC
                    ↓
                Consumer processes
                    ↓
                processed_events
                    ↓
                RabbitMQ accidentally delivers #ABC again
                    ↓
                Consumer checks processed_events
                    ↓
                Already processed → ignore