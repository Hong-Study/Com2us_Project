USE ApiDatabase;

CREATE TABLE user_game_data(
    user_id VARCHAR(50) PRIMARY KEY,
    user_name VARCHAR(30) NOT NULL,
    level INT NOT NULL,
    exp INT NOT NULL,
    gold INT NOT NULL,
    win INT NOT NULL,
    lose INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    INDEX (user_name)
);

CREATE TABLE user_attendance_data(
    user_id VARCHAR(50) NOT NULL,
    attendance_date DATE NOT NULL,
    is_success BOOLEAN NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES user_game_data(user_id)
);

CREATE TABLE item_data(
    item_id BIGINT PRIMARY KEY,
    item_name VARCHAR(255) NOT NULL,
    item_type VARCHAR(255) NOT NULL,
    item_price INT NOT NULL
);

CREATE TABLE mail_data(
    mail_id SERIAL PRIMARY KEY,
    send_user_name VARCHAR(30) NOT NULL,
    recv_user_name VARCHAR(30) NOT NULL,
    mail_title VARCHAR(30) NOT NULL,
    mail_content TEXT NOT NULL,
    -- mail_item_id INT,
    -- mail_item_count INT, 
    is_read BOOLEAN NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);