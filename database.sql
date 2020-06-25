CREATE TABLE `banks` (
	`name` VARCHAR(50) NOT NULL,
	`money` DOUBLE NULL DEFAULT 0,
	`gold` DOUBLE NULL DEFAULT 0,
	PRIMARY KEY (`name`)
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;


CREATE TABLE `bank_users` (
	`name` VARCHAR(50) NOT NULL,
	`identifier` VARCHAR(50) NOT NULL,
	`money` DOUBLE NULL DEFAULT 0,
	`gold` DOUBLE NULL DEFAULT 0,
	PRIMARY KEY (`identifier`),
	INDEX `name` (`name`),
	CONSTRAINT `bank` FOREIGN KEY (`name`) REFERENCES `banks` (`name`) ON DELETE CASCADE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;
