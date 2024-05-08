USE HiveDatabase;

CREATE TABLE user_data(
    user_id VARCHAR(50) PRIMARY KEY,
    password VARCHAR(100) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);