CREATE DATABASE mystiick;
GO

USE `mystiick`;

CREATE TABLE `Image` (
  `ImageID` int unsigned NOT NULL AUTO_INCREMENT,
  `GUID` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ImagePath` varchar(400) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `PreviewPath` varchar(400) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ThumbnailPath` varchar(400) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Category` varchar(400) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `SubCategory` varchar(400) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `Created` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`ImageID`),
  UNIQUE KEY `U_GUID` (`GUID`),
  KEY `IX_GUID` (`GUID`)
) ENGINE=InnoDB DEFAULT CHARSET=ascii;


CREATE TABLE `ImageSettings` (
  `ImageSettingsID` int unsigned NOT NULL AUTO_INCREMENT,
  `ImageID` int unsigned NOT NULL,
  `Model` varchar(45) DEFAULT NULL,
  `Flash` tinyint(1) DEFAULT NULL,
  `ISO` smallint DEFAULT NULL,
  `ShutterSpeed` varchar(45) DEFAULT NULL,
  `Aperature` varchar(45) DEFAULT NULL,
  `FocalLength` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`ImageSettingsID`),
  KEY `ImageSettings_Image_idx` (`ImageID`),
  CONSTRAINT `ImageSettings_Image` FOREIGN KEY (`ImageID`) REFERENCES `Image` (`ImageID`)
) ENGINE=InnoDB DEFAULT CHARSET=ascii;


CREATE TABLE `ImageTag` (
  `ImageTagID` int unsigned NOT NULL AUTO_INCREMENT,
  `ImageID` int unsigned NOT NULL,
  `TagName` varchar(400) NOT NULL,
  PRIMARY KEY (`ImageTagID`),
  KEY `FK_ImageTag_Image` (`ImageID`),
  CONSTRAINT `FK_ImageTag_Image` FOREIGN KEY (`ImageID`) REFERENCES `Image` (`ImageID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=ascii;


SET NAMES utf8mb4;

CREATE TABLE `Link` (
  `LinkID` int unsigned NOT NULL AUTO_INCREMENT,
  `LinkText` tinytext NOT NULL,
  `LinkUrl` text NOT NULL,
  `Icon` tinytext NOT NULL,
  PRIMARY KEY (`LinkID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE `Post` (
  `PostID` int unsigned NOT NULL AUTO_INCREMENT,
  `PostType` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PostTitle` text NOT NULL,
  `PostText` text NOT NULL,
  `Created` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`PostID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE `PostAttachment` (
  `PostAttachmentID` int unsigned NOT NULL AUTO_INCREMENT,
  `PostID` int unsigned NOT NULL,
  `AttachmentType` varchar(25) NOT NULL,
  `ObjectID` int NOT NULL,
  PRIMARY KEY (`PostAttachmentID`),
  KEY `PostID` (`PostID`),
  CONSTRAINT `PostAttachment_ibfk_1` FOREIGN KEY (`PostID`) REFERENCES `Post` (`PostID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
