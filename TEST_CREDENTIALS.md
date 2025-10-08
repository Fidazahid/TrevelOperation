# Travel Operation - Test User Credentials

This document contains all the mock user credentials for testing the authentication system with different user roles.

## Authentication System Overview

The system supports three user roles with different access levels:

1. **Finance Managers** (1-2 users) - Full access to everything
2. **Department Owners** (5-10 users) - Access to their department's data only  
3. **Employees** (50-200+ users) - Access to personal data only

## Test Credentials

### 🏦 FINANCE MANAGERS (Full System Access)
*Can access all features, all departments, all settings*

| Email | Password | Department |
|-------|----------|------------|
| admin@noras.com | admin123 | Finance |
| martina.popinsk@wsc.com | finance123 | Finance |
| maayan.chesler@wsc.com | finance123 | Finance |
| ceo@wsc.com | exec123 | Executive |
| cfo@wsc.com | exec123 | Executive |
| cto@wsc.com | exec123 | Executive |

---

### 👨‍💼 DEPARTMENT OWNERS (Department-Level Access)
*Can access their department's data, trip validation, travel spend reports*

#### Sales Department
| Email | Password | Department |
|-------|----------|------------|
| sales.manager@wsc.com | manager123 | Sales |
| sales.director@wsc.com | manager123 | Sales |

#### IT Department  
| Email | Password | Department |
|-------|----------|------------|
| it.manager@wsc.com | manager123 | IT |
| tech.lead@wsc.com | manager123 | IT |

#### HR Department
| Email | Password | Department |
|-------|----------|------------|
| hr.manager@wsc.com | manager123 | HR |
| hr.director@wsc.com | manager123 | HR |

#### Operations Department
| Email | Password | Department |
|-------|----------|------------|
| ops.manager@wsc.com | manager123 | Operations |
| operations.head@wsc.com | manager123 | Operations |

#### Development Department
| Email | Password | Department |
|-------|----------|------------|
| dev.manager@wsc.com | manager123 | Development |
| engineering.lead@wsc.com | manager123 | Development |

#### Marketing Department
| Email | Password | Department |
|-------|----------|------------|
| marketing.manager@wsc.com | manager123 | Marketing |

#### Legal Department
| Email | Password | Department |
|-------|----------|------------|
| legal.counsel@wsc.com | manager123 | Legal |

#### Customer Support
| Email | Password | Department |
|-------|----------|------------|
| support.manager@wsc.com | manager123 | Support |

---

### 👤 EMPLOYEES (Personal Data Access Only)
*Can only access their own transactions and trips*

#### Sales Department (10 employees)
| Email | Password | Department |
|-------|----------|------------|
| john.doe@wsc.com | emp123 | Sales |
| sarah.wilson@wsc.com | emp123 | Sales |
| mike.johnson@wsc.com | emp123 | Sales |
| lisa.brown@wsc.com | emp123 | Sales |
| david.miller@wsc.com | emp123 | Sales |
| emma.davis@wsc.com | emp123 | Sales |
| alex.garcia@wsc.com | emp123 | Sales |
| kelly.martinez@wsc.com | emp123 | Sales |
| ryan.anderson@wsc.com | emp123 | Sales |
| nicole.taylor@wsc.com | emp123 | Sales |

#### IT Department (10 employees)
| Email | Password | Department |
|-------|----------|------------|
| jane.smith@wsc.com | emp123 | IT |
| tom.wilson@wsc.com | emp123 | IT |
| amy.chen@wsc.com | emp123 | IT |
| carlos.rodriguez@wsc.com | emp123 | IT |
| priya.patel@wsc.com | emp123 | IT |
| james.lee@wsc.com | emp123 | IT |
| maria.gonzalez@wsc.com | emp123 | IT |
| kevin.kim@wsc.com | emp123 | IT |
| jessica.wang@wsc.com | emp123 | IT |
| daniel.park@wsc.com | emp123 | IT |

#### HR Department (6 employees)
| Email | Password | Department |
|-------|----------|------------|
| bob.wilson@wsc.com | emp123 | HR |
| jennifer.clark@wsc.com | emp123 | HR |
| robert.lewis@wsc.com | emp123 | HR |
| michelle.white@wsc.com | emp123 | HR |
| steven.hall@wsc.com | emp123 | HR |
| laura.green@wsc.com | emp123 | HR |

#### Operations Department (8 employees)
| Email | Password | Department |
|-------|----------|------------|
| alice.johnson@wsc.com | emp123 | Operations |
| chris.adams@wsc.com | emp123 | Operations |
| sandra.baker@wsc.com | emp123 | Operations |
| mark.evans@wsc.com | emp123 | Operations |
| patricia.hill@wsc.com | emp123 | Operations |
| william.scott@wsc.com | emp123 | Operations |
| nancy.carter@wsc.com | emp123 | Operations |
| joseph.mitchell@wsc.com | emp123 | Operations |

#### Development Department (10 employees)
| Email | Password | Department |
|-------|----------|------------|
| charlie.brown@wsc.com | emp123 | Development |
| anna.thompson@wsc.com | emp123 | Development |
| brian.moore@wsc.com | emp123 | Development |
| rachel.jackson@wsc.com | emp123 | Development |
| andrew.martin@wsc.com | emp123 | Development |
| stephanie.lee@wsc.com | emp123 | Development |
| derek.wright@wsc.com | emp123 | Development |
| vanessa.lopez@wsc.com | emp123 | Development |
| nathan.harris@wsc.com | emp123 | Development |
| crystal.clark@wsc.com | emp123 | Development |

#### Marketing Department (8 employees)
| Email | Password | Department |
|-------|----------|------------|
| karen.walker@wsc.com | emp123 | Marketing |
| gary.young@wsc.com | emp123 | Marketing |
| helen.king@wsc.com | emp123 | Marketing |
| paul.wright@wsc.com | emp123 | Marketing |
| diane.lopez@wsc.com | emp123 | Marketing |
| roger.hill@wsc.com | emp123 | Marketing |
| julie.green@wsc.com | emp123 | Marketing |
| terry.adams@wsc.com | emp123 | Marketing |

#### Finance Department (3 employees - non-managers)
| Email | Password | Department |
|-------|----------|------------|
| susan.financial@wsc.com | emp123 | Finance |
| peter.accountant@wsc.com | emp123 | Finance |
| linda.analyst@wsc.com | emp123 | Finance |

#### Legal Department (2 employees)
| Email | Password | Department |
|-------|----------|------------|
| attorney.smith@wsc.com | emp123 | Legal |
| paralegal.jones@wsc.com | emp123 | Legal |

#### Customer Support (3 employees)
| Email | Password | Department |
|-------|----------|------------|
| agent.one@wsc.com | emp123 | Support |
| agent.two@wsc.com | emp123 | Support |
| agent.three@wsc.com | emp123 | Support |

---

## Quick Test Scenarios

### Test Finance Manager Access
```
Email: admin@noras.com
Password: admin123
Expected: Full access to all features, all menus visible
```

### Test Department Owner Access  
```
Email: sales.manager@wsc.com
Password: manager123
Expected: Access to sales department data, limited menus
```

### Test Employee Access
```
Email: john.doe@wsc.com  
Password: emp123
Expected: Personal data only, basic menus
```

## Navigation Menu Expectations

### Finance Manager
- ✅ Dashboard
- ✅ Reports (Transactions, Trips, Travel Spend)
- ✅ Trip Management (Create, Suggestions, Validation)
- ✅ Data Integrity (Controls, Matching, Split)
- ✅ Settings (All options)

### Department Owner
- ✅ Dashboard
- ✅ Reports (Transactions, Trips, Travel Spend for their department)
- ✅ Trip Management (Create, Suggestions, Validation for their department)
- ❌ Data Integrity (Not accessible)
- ❌ Settings (Not accessible)

### Employee
- ✅ Dashboard
- ✅ Reports (Personal Transactions, Trips only)
- ✅ Trip Management (Create, Suggestions for personal trips)
- ❌ Data Integrity (Not accessible)
- ❌ Settings (Not accessible)

---

## Session Management

- **Session Duration**: 24 hours
- **Storage**: localStorage
- **Auto-redirect**: Users are redirected to login if session expires
- **State Management**: Real-time authentication state updates across components