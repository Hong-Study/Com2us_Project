USE ApiDatabase;

CREATE TABLE user_game_data(
    user_id INT PRIMARY KEY,
    user_name VARCHAR(30) NOT NULL,
    level INT NOT NULL,
    exp INT NOT NULL,
    gold INT NOT NULL,
    win INT NOT NULL,
    lose INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE user_attendance_data(
    user_id INT,
    attendance_date DATE NOT NULL,
    is_success BOOLEAN NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES user_game_data(user_id)
);

CREATE TABLE user_item_data(
    item_id INT PRIMARY KEY,
    user_id INT NOT NULL,
    item_name VARCHAR(255) NOT NULL,
    item_type VARCHAR(255) NOT NULL,
    item_price INT NOT NULL,
    item_count INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES user_game_data(user_id)
);

CREATE TABLE mail_data(
    mail_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    send_user_id INT NOT NULL,
    mail_title VARCHAR(255) NOT NULL,
    mail_content TEXT NOT NULL,
    mail_item_id INT,
    is_read BOOLEAN NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES user_game_data(user_id),
    FOREIGN KEY (send_user_id) REFERENCES user_game_data(user_id),
    FOREIGN KEY (mail_item_id) REFERENCES user_item_data(item_id)
);