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
  `ImageSettingsID` int NOT NULL AUTO_INCREMENT,
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
